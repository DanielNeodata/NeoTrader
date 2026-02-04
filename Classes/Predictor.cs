namespace NeoTrader.Classes
{
	public class Predictor
	{
		public MLContext mlContext = new MLContext();
		public ITransformer trainedModel;
		private static uint ExperimentTime = 30;
		private static string LabelColumnName = "Open";

		public void Load(List<PredictorInputData> trainning) {
			IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(trainning);
			ExperimentResult<RegressionMetrics> experimentResult = mlContext.Auto().CreateRegressionExperiment(ExperimentTime).Execute(trainingDataView, LabelColumnName);
			RunDetail<RegressionMetrics> best = experimentResult.BestRun;
			trainedModel = best.Model;
		}

		public PredictorOutputData Predict(PredictorInputData _actualState) {
			PredictionEngine<PredictorInputData, PredictorOutputData> predEngine = mlContext.Model.CreatePredictionEngine<PredictorInputData, PredictorOutputData>(trainedModel);
			return predEngine.Predict(_actualState);
		}
	}
}

