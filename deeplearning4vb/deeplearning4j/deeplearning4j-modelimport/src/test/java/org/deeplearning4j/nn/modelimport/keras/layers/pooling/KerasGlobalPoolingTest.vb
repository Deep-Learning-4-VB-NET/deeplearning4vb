Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals

Namespace org.deeplearning4j.nn.modelimport.keras.layers.pooling

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Global Pooling Tests") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class KerasGlobalPoolingTest extends org.deeplearning4j.BaseDL4JTest
	Public Class KerasGlobalPoolingTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPoolingNWHC() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPoolingNWHC()
			Dim absolutePath As String = Resources.asFile("modelimport/keras/tfkeras/GAPError.h5").getAbsolutePath()
			Dim computationGraph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(absolutePath)
			Dim sampleInput As INDArray = Nd4j.ones(1,400,128)
			Dim output() As INDArray = computationGraph.output(sampleInput)
			assertArrayEquals(New Long(){1, 400, 512},output(0).shape())

		End Sub

	End Class

End Namespace