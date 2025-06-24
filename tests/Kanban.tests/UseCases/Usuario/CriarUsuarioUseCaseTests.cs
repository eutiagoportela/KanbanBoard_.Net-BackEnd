using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Kanban.Aplicacao.UseCases.Usuario.CriarUsuario;
using Kanban.Comunicacao.Requests.Usuario;
using Kanban.Comunicacao.Responses.Usuario;
using Kanban.Dominio.Entidades;
using Kanban.Dominio.Repositorios.Interfaces;
using Kanban.Exceptions;
using Kanban.Infraestrutura.Security;

namespace Kanban.tests.UseCases.Usuario
{
    public class CriarUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CriarUsuarioUseCase>> _loggerMock;
        private readonly CriarUsuarioUseCase _useCase;

        public CriarUsuarioUseCaseTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CriarUsuarioUseCase>>();

            _useCase = new CriarUsuarioUseCase(
                _usuarioRepositoryMock.Object,
                _passwordHasherMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_DeveCriarUsuario_ComDadosValidos()
        {
            // Arrange
            var request = new CriarUsuarioRequest
            {
                Nome = "Tiago",
                Email = "tiago@teste.com",
                Senha = "123456"
            };

            var senhaHash = "hash123";
            _passwordHasherMock.Setup(x => x.HashPassword(request.Senha)).Returns(senhaHash);
            _usuarioRepositoryMock.Setup(x => x.EmailExisteAsync(request.Email)).ReturnsAsync(false);

            var usuarioCriado = new Usuarios
            {
                Id = 1,
                Nome = request.Nome,
                Email = request.Email,
                SenhaHash = senhaHash
            };

            _mapperMock.Setup(x => x.Map<UsuarioResponse>(It.IsAny<Usuarios>()))
                .Returns(new UsuarioResponse { Id = 1, Nome = request.Nome, Email = request.Email });

            // Act
            var result = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            _usuarioRepositoryMock.Verify(x => x.CriarAsync(It.IsAny<Usuarios>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_DeveLancarExcecao_SeEmailExistir()
        {
            // Arrange
            var request = new CriarUsuarioRequest
            {
                Nome = "Tiago",
                Email = "tiago@teste.com",
                Senha = "123456"
            };

            _usuarioRepositoryMock.Setup(x => x.EmailExisteAsync(request.Email)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<EmailJaExisteException>(() => _useCase.ExecuteAsync(request));
        }

        [Theory]
        [InlineData("", "senha123", "nome")]
        [InlineData("email@teste.com", "", "nome")]
        [InlineData("email@teste.com", "123", "nome")]
        [InlineData("email@teste.com", "senha123", "")]
        public async Task ExecuteAsync_DeveLancarValidacaoException_SeDadosForemInvalidos(string email, string senha, string nome)
        {
            // Arrange
            var request = new CriarUsuarioRequest
            {
                Nome = nome,
                Email = email,
                Senha = senha
            };

            _usuarioRepositoryMock.Setup(x => x.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ValidacaoException>(() => _useCase.ExecuteAsync(request));
        }
    }
}
