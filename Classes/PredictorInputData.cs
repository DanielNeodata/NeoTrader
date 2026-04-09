using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers.LightGbm;

namespace NeoTrader.Classes
{
	public class PredictorInputData
	{
		[ColumnName("DatePrice"), LoadColumn(0)]
		public float DatePrice { get; set; }

		[ColumnName("Open"), LoadColumn(1)]
		public float Open { get; set; }

		[ColumnName("Close"), LoadColumn(2)]
		public float Close { get; set; }

		[ColumnName("Volume"), LoadColumn(3)]
		public float Volume { get; set; }

		[ColumnName("TwoWeeks"), LoadColumn(4)]
		public float TwoWeeks { get; set; }

		[ColumnName("MarketDay"), LoadColumn(5)]
		public float MarketDay { get; set; }

		[ColumnName("LowHigh"), LoadColumn(6)]
		public float LowHigh { get; set; }

		[ColumnName("PercentageMovementPreviousDay"), LoadColumn(7)]
		public float PercentageMovementPreviousDay { get; set; }

		[ColumnName("PercentageMovementPreviousWeek"), LoadColumn(8)]
		public float PercentageMovementPreviousWeek { get; set; }

		[ColumnName("PercentageMovementPreviousMonth"), LoadColumn(9)]
		public float PercentageMovementPreviousMonth { get; set; }

		[ColumnName("ModificadorBaseGauss"), LoadColumn(10)]
		public float ModificadorBaseGauss { get; set; }

		[ColumnName("ModificadorMaterialGauss"), LoadColumn(11)]
		public float ModificadorMaterialGauss { get; set; }

		[ColumnName("ModificadorFinalGauss"), LoadColumn(12)]
		public float ModificadorFinalGauss { get; set; }

		[ColumnName("Label"), LoadColumn(13)]
		public Boolean Label { get; set; }
	}
	public class MLOptions
	{
		public string[] featureColumns = new string[] { "DatePrice,Open,Close,Volume,TwoWeeks,MarketDay,LowHigh,PercentageMovementPreviousDay,PercentageMovementPreviousWeek,PercentageMovementPreviousMonth,ModificadorBaseGauss,ModificadorMaterialGauss,ModificadorFinalGauss" };
		public string modelTraining = "";

		public SdcaLogisticRegressionBinaryTrainer.Options _sdca_LR_Options = new SdcaLogisticRegressionBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
			Shuffle = true,
			L1Regularization = 0f,
			MaximumNumberOfIterations = 20
		};
		public SdcaNonCalibratedBinaryTrainer.Options _sdca_NC_Options = new SdcaNonCalibratedBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
			Shuffle = true
		};
		public SgdCalibratedTrainer.Options _sgd_CT_Options = new SgdCalibratedTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
			Shuffle = true
		};
		public SgdNonCalibratedTrainer.Options _sgd_NC_Options = new SgdNonCalibratedTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
			Shuffle = true
		};
		public FastTreeBinaryTrainer.Options _ft_bt_Options = new FastTreeBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public FastForestBinaryTrainer.Options _ff_bt_Options = new FastForestBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public FieldAwareFactorizationMachineTrainer.Options _fa_fm_Options = new()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public LightGbmBinaryTrainer.Options _lgbm_bt_Options = new LightGbmBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public GamBinaryTrainer.Options _gam_bt_Options = new GamBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public LbfgsLogisticRegressionBinaryTrainer.Options _lbfg_bt_Options = new LbfgsLogisticRegressionBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public LinearSvmTrainer.Options _lsvm_T_Options = new LinearSvmTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
		public SymbolicSgdLogisticRegressionBinaryTrainer.Options _ssgd_rt_Options = new SymbolicSgdLogisticRegressionBinaryTrainer.Options()
		{
			LabelColumnName = "Label",
			FeatureColumnName = "Features",
		};
	}
}

