
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

            app.MapGet("/iris", (float sepalLength, float sepalWidth, float petalLength, float petalWidth) =>
            {
                var sampleData = new MLIrisModel.ModelInput()
                {
                    Sepal_Length = sepalLength,
                    Sepal_Width = sepalWidth,
                    Petal_Length = petalLength,
                    Petal_Width = petalWidth,
                };

                var result = MLIrisModel.Predict(sampleData);

                var species = result.PredictedLabel.Replace(" ", "").ToLower().Replace("-", ""); // make filename safe
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "img", $"{species}.jpeg");

                string? base64Image = null;
                if (File.Exists(filePath))
                {
                    var bytes = File.ReadAllBytes(filePath);
                    base64Image = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                }

                return Results.Json(new
                {
                    SepalLength = sepalLength,
                    SepalWidth = sepalWidth,
                    PetalLength = petalLength,
                    PetalWidth = petalWidth,
                    PredictedSpecies = result.PredictedLabel,
                    positiveProbability = (result.Score.Max() * 100).ToString("0.00") + "%",
                    negativeProbability = (result.Score.Min() * 100).ToString("0.00") + "%",
                    ImageBase64 = base64Image
                });
            })
            .WithName("GetIris")
            .WithOpenApi();

            app.Run();
        }
    }
}
