
namespace IrisAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapPost("/iris", (IrisInput input) =>
            {
                try
                {
                    var sampleData = new MLIrisModel.ModelInput()
                    {
                        Sepal_Length = input.SepalLength,
                        Sepal_Width = input.SepalWidth,
                        Petal_Length = input.PetalLength,
                        Petal_Width = input.PetalWidth,
                    };

                    var result = MLIrisModel.Predict(sampleData);

                    var species = result.PredictedLabel.Replace(" ", "").ToLower().Replace("-", "");
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "img", $"{species}.jpeg");

                    string? base64Image = null;
                    if (File.Exists(filePath))
                    {
                        var bytes = File.ReadAllBytes(filePath);
                        base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                    }

                    return Results.Json(new
                    {
                        SepalLength = input.SepalLength,
                        SepalWidth = input.SepalWidth,
                        PetalLength = input.PetalLength,
                        PetalWidth = input.PetalWidth,
                        PredictedSpecies = result.PredictedLabel,
                        positiveProbability = (result.Score.Max() * 100).ToString("0.00") + "%",
                        negativeProbability = (result.Score.Min() * 100).ToString("0.00") + "%",
                        ImageBase64 = base64Image
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            })
            .WithName("PostIris")
            .WithOpenApi();

            app.Run();
        }
    }
}
