using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevIO.App.Data;
using DevIO.App.ViewModels;
using DevIO.App.Interface;
using AutoMapper;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using DevIO.App.Notificacoes;
using Microsoft.AspNetCore.Authorization;
using DevIO.App.Extensions;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoRepository produtoRepository, 
                                  IFornecedorRepository fornecedorRepository, 
                                  IProdutoService produtoService,
                                  INotificador notificador,
                                  IMapper mapper) : base(mapper, notificador)
        {
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;
            _produtoService = produtoService;
        }

        [Route("lista-de-produto")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
        }

        [Route("detalhes-do-produto/{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {

            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();
            return View(produtoViewModel);
        }

        [Route("novo-produto")]
        [ClaimsAuthorize("Produtos","Adicionar")]
        public async Task<IActionResult> Create()
        {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());
            return View(produtoViewModel);
        }

        [HttpPost]
        [Route("novo-produto/{produtoViewModel}")]
        [ClaimsAuthorize("Produtos", "Adicionar")]
        public async Task<IActionResult> Create( ProdutoViewModel produtoViewModel)
        {
            produtoViewModel = await PopularFornecedores(produtoViewModel);
            var imgPrefixo = $"{Guid.NewGuid()}_";

            if(! await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo)) return View(produtoViewModel);
            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;

            if (!ModelState.IsValid) return View(produtoViewModel);
            
            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtoService.Adicionar(produto);

            if(!OperacaoValida()) return View(produtoViewModel);
            return RedirectToAction(nameof(Index));


        }
        [Route("editar-produto/{id:guid}")]
        [ClaimsAuthorize("Produtos", "Editar")]
        public async Task<IActionResult> Edit(Guid id)
        {
            
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();
            return View(produtoViewModel);
        }

        [HttpPost]
        [Route("editar-produto/{id:guid}")]
        [ClaimsAuthorize("Produtos", "Editar")]
        public async Task<IActionResult> Edit(Guid id,  ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id) return NotFound();

            var produtoAtualizacao = await ObterProduto(id);

            produtoViewModel.Fornecedor = produtoAtualizacao.Fornecedor;
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return View(produtoViewModel);

            if(produtoViewModel.ImagemUpload != null)
            {
                var imgPrefixo = $"{Guid.NewGuid()}_";

                if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo)) return View(produtoViewModel);

                produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            produtoAtualizacao.Fornecedor = null;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            if (!OperacaoValida()) return View(produtoViewModel);

            return RedirectToAction(nameof(Index));

        }

        [Route("excluir-produto/{id:guid}")]
        [ClaimsAuthorize("Produtos", "Excluir")]
        public async Task<IActionResult> Delete(Guid id)
        {          

            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();
            return View(produtoViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [Route("excluir-produto/{id:guid}")]
        [ClaimsAuthorize("Produtos", "Excluir")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();

            await _produtoService.Remover(id);

            if (!OperacaoValida()) return View(produtoViewModel);

            TempData["Sucesso"] = "Produto excluído com sucesso!";

            return RedirectToAction(nameof(Index), TempData["Sucesso"]);
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produto;
        }

        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produto)
        {
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produto;
        }

        private async Task<bool> UploadArquivo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo.Length <= 0) return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            if(System.IO.File.Exists(path))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
                return false;
            }
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            return true;
        }


    }
}
