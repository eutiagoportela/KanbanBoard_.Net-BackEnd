using AutoMapper;
using Kanban.Aplicacao.UseCases.Tarefa.CriarTarefa;
using Kanban.Comunicacao.Requests.Tarefa;
using Kanban.Comunicacao.Responses.Tarefa;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Enum;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Kanban.Tests.UseCases.Tarefa
{
    public class CriarTarefaUseCaseTests
    {
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock = new();
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly CriarTarefaUseCase _useCase;

        public CriarTarefaUseCaseTests()
        {
            _useCase = new CriarTarefaUseCase(
                _tarefaRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_DeveCriarTarefa_QuandoUsuarioExiste()
        {
            // Arrange
            int usuarioId = 1;
            var request = new CriarTarefaRequest
            {
                Titulo = "Tarefa Teste",
                Descricao = "Descrição",
                Status = StatusTarefa.AFazer,
                DataVencimento = DateTime.UtcNow.AddDays(3)
            };


            var usuario = new Usuarios { Id = usuarioId };
            var tarefaCriada = new Tarefas
            {
                Id = 10,
                Titulo = request.Titulo,
                UsuarioId = usuarioId
            };
            var respostaEsperada = new TarefaResponse
            {
                Id = tarefaCriada.Id,
                Titulo = tarefaCriada.Titulo
            };

            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(usuarioId))
                                  .ReturnsAsync(usuario);

            _tarefaRepositoryMock.Setup(r => r.CriarAsync(It.IsAny<Tarefas>()))
                                 .ReturnsAsync(tarefaCriada);

            _mapperMock.Setup(m => m.Map<TarefaResponse>(It.IsAny<Tarefas>()))
                       .Returns(respostaEsperada);

            // Act
            var resultado = await _useCase.ExecuteAsync(usuarioId, request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(respostaEsperada.Id, resultado.Id);
            Assert.Equal(respostaEsperada.Titulo, resultado.Titulo);

            _usuarioRepositoryMock.Verify(r => r.ObterPorIdAsync(usuarioId), Times.Once);
            _tarefaRepositoryMock.Verify(r => r.CriarAsync(It.IsAny<Tarefas>()), Times.Once);
            _mapperMock.Verify(m => m.Map<TarefaResponse>(It.IsAny<Tarefas>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
        {
            // Arrange
            int usuarioId = 99;
            var request = new CriarTarefaRequest
            {
                Titulo = "Tarefa Teste",
                Descricao = "Descrição",
                Status = StatusTarefa.AFazer,
                DataVencimento = DateTime.UtcNow.AddDays(3)
            };


            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(usuarioId))
                                  .ReturnsAsync((Usuarios)null);

            // Act & Assert
            await Assert.ThrowsAsync<UsuarioNaoEncontradoException>(() =>
                _useCase.ExecuteAsync(usuarioId, request));

            _usuarioRepositoryMock.Verify(r => r.ObterPorIdAsync(usuarioId), Times.Once);
            _tarefaRepositoryMock.Verify(r => r.CriarAsync(It.IsAny<Tarefas>()), Times.Never);
        }
    }
}
