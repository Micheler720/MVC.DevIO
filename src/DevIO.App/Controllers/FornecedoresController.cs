using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevIO.App.ViewModels;
using DevIO.App.Interface;
using AutoMapper;
using DevIO.Business.Models;
using DevIO.App.Notificacoes;
using Microsoft.AspNetCore.Authorization;
using DevIO.App.Extensions;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class FornecedoresController : BaseController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IEnderecoRepository enderecoRepository,
                                      IFornecedorService fornecedorService,
                                      INotificador notificador,
                                      IMapper mapper) : base(mapper, notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _fornecedorService = fornecedorService;
        }

        [Route("lista-de-fornecedores")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<FornecedorViewModel>>( await _fornecedorRepository.ObterTodos()));
        }

        [Route("dados-do-fornecedores/{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {

            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null)
            {
                return NotFound();
            }
            return View(fornecedorViewModel);
        }

        [Route("novo-fornecedor")]
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("novo-fornecedor")]
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        public async Task<IActionResult> Create( FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            await _fornecedorService.Adicionar(fornecedor);
            if (!OperacaoValida()) return View(fornecedorViewModel);

            return RedirectToAction(nameof(Index));
        }


        [Route("editar-fornecedor/{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Editar")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var fornecedorViewModel = await  ObterFornecedorProdutosEndereco(id);
            if (fornecedorViewModel == null)
            {
                return NotFound();
            }
            return View(fornecedorViewModel);
        }

        
        [HttpPost]
        [Route("editar-fornecedor/{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Editar")]
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            await _fornecedorService.Atualizar(fornecedor);
            if (!OperacaoValida()) return View(fornecedorViewModel);

            return RedirectToAction(nameof(Index));
            
        }

        [Route("excluir-fornecedor/{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        public async Task<IActionResult> Delete(Guid id)
        {
           
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            return View(fornecedorViewModel);
        }

        [Route("excluir-fornecedor/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);
            if (fornecedorViewModel == null) return NotFound();

            await _fornecedorService.Remover(id);
            if (!OperacaoValida()) return View(fornecedorViewModel);

            return RedirectToAction(nameof(Index));
        }

        [Route("obter-endereco-fornecedor/{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);
            if (fornecedor == null) return NotFound();

            return PartialView("_DetalhesEndereco", fornecedor);
        }

        [HttpGet]
        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Editar")]
        public async Task<IActionResult> AtualizarEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null) return NotFound();

            return PartialView("_AtualizarEndereco", new FornecedorViewModel { Endereco = fornecedor.Endereco });
        }

        [HttpPost]
        [Route("obter-endereco-fornecedor/{fornecedorViewModel}")]
        [ClaimsAuthorize("Fornecedor", "Editar")]
        public async Task<IActionResult> AtualizarEndereco(FornecedorViewModel fornecedorViewModel)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");
            if(!ModelState.IsValid) return PartialView("_AtualizarEndereco", fornecedorViewModel);
            var endereco = _mapper.Map<Endereco>(fornecedorViewModel.Endereco);

            await _fornecedorService.AtualizarEndereco(endereco);
            if (!OperacaoValida()) return View(fornecedorViewModel);

            var url = Url.Action("ObterEndereco", "Fornecedores", new { id = fornecedorViewModel.Endereco.FornecedorId });

            return Json(new { success = true, url });
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

         
    }
}
