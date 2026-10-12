Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports CudnnConvolutionHelper = org.deeplearning4j.cuda.convolution.CudnnConvolutionHelper
Imports CudnnSubsamplingHelper = org.deeplearning4j.cuda.convolution.subsampling.CudnnSubsamplingHelper
Imports CudnnDropoutHelper = org.deeplearning4j.cuda.dropout.CudnnDropoutHelper
Imports CudnnLocalResponseNormalizationHelper = org.deeplearning4j.cuda.normalization.CudnnLocalResponseNormalizationHelper
Imports CudnnLSTMHelper = org.deeplearning4j.cuda.recurrent.CudnnLSTMHelper
Imports DropoutHelper = org.deeplearning4j.nn.conf.dropout.DropoutHelper
Imports HelperUtils = org.deeplearning4j.nn.layers.HelperUtils
Imports ConvolutionHelper = org.deeplearning4j.nn.layers.convolution.ConvolutionHelper
Imports SubsamplingHelper = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingHelper
Imports LocalResponseNormalizationHelper = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalizationHelper
Imports LSTMHelper = org.deeplearning4j.nn.layers.recurrent.LSTMHelper
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
import static org.junit.jupiter.api.Assertions.assertNotNull

Namespace org.deeplearning4j.cuda


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class HelperUtilsTests extends org.deeplearning4j.BaseDL4JTest
	Public Class HelperUtilsTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHelperCreation()
		Public Overridable Sub testHelperCreation()
			System.setProperty(DL4JSystemProperties.DISABLE_HELPER_PROPERTY,"false")

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper(GetType(CudnnDropoutHelper).FullName,"", GetType(DropoutHelper),"layer-name",DataType))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper(GetType(CudnnConvolutionHelper).FullName,"", GetType(ConvolutionHelper),"layer-name",DataType))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper(GetType(CudnnLocalResponseNormalizationHelper).FullName,"", GetType(LocalResponseNormalizationHelper),"layer-name",DataType))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper(GetType(CudnnSubsamplingHelper).FullName,"", GetType(SubsamplingHelper),"layer-name",DataType))

		End Sub

	End Class

End Namespace