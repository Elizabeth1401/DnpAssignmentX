// See https://aka.ms/new-console-template for more information

using CLI.UI;
using FileRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI application...");
IUserRepository userRepository = new UserFileRepository(); // old: UserInMemoryRepository();
ICommentRepository commentRepository = new CommentFileRepository(); // old: CommentInMemoryRepository();
IPostRepository postRepository = new PostFileRepository(); // old: PostInMemoryRepository();

CliApp cliApp = new CliApp(userRepository, postRepository,commentRepository);
await cliApp.StartAsync();