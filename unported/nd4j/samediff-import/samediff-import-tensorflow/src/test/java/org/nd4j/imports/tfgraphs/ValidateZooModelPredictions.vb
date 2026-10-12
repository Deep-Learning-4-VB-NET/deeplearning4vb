Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.imports.tfgraphs


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class ValidateZooModelPredictions extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ValidateZooModelPredictions
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.create(1)
			Nd4j.DataType = DataType.DOUBLE
			Nd4j.Random.setSeed(123)
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMobilenetV1(@TempDir Path testDir,org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMobilenetV1(ByVal testDir As Path, ByVal backend As Nd4jBackend)
			TFGraphTestZooModels.currentTestDir = testDir.toFile()

			'Load model
			Dim path As String = "tf_graphs/zoo_models/mobilenet_v1_0.5_128/tf_model.txt"
			Dim resource As File = (New ClassPathResource(path)).File
			Dim sd As SameDiff = TFGraphTestZooModels.LOADER.apply(resource, "mobilenet_v1_0.5_128")

			'Load data
			'Because we don't have DataVec NativeImageLoader in ND4J tests due to circular dependencies, we'll load the image previously saved...
			Dim imgFile As File = (New ClassPathResource("deeplearning4j-zoo/goldenretriever_rgb128_unnormalized_nchw_INDArray.bin")).File
			Dim img As INDArray = Nd4j.readBinary(imgFile).castTo(DataType.FLOAT)
			img = img.permute(0,2,3,1).dup() 'to NHWC

			'Mobilenet V1 - not sure, but probably using inception preprocessing
			'i.e., scale to (-1,1) range
			'Image is originally 0 to 255
			img.divi(255).subi(0.5).muli(2)

			Dim min As Double = img.minNumber().doubleValue()
			Dim max As Double = img.maxNumber().doubleValue()

			assertTrue(min >= -1 AndAlso min <= -0.6)
			assertTrue(max <= 1 AndAlso max >= 0.6)

			'Perform inference
			Dim inputs As IList(Of String) = sd.inputs()
			assertEquals(1, inputs.Count)

			Dim [out] As String = "MobilenetV1/Predictions/Softmax"
			Dim m As IDictionary(Of String, INDArray) = sd.output(Collections.singletonMap(inputs(0), img), [out])

			Dim outArr As INDArray = m([out])


			Console.WriteLine("SHAPE: " & java.util.Arrays.toString(outArr.shape()))
			Console.WriteLine(outArr)

			Dim argmax As INDArray = outArr.argMax(1)

			'Load labels
			Dim labels As IList(Of String) = ValidateZooModelPredictions.labels()

			Dim classIdx As Integer = argmax.getInt(0)
			Dim className As String = labels(classIdx)
			Dim expClass As String = "golden retriever"
			Dim prob As Double = outArr.getDouble(classIdx)

			Console.WriteLine("Predicted class: """ & className & """ - probability = " & prob)
			assertEquals(expClass, className)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResnetV2(@TempDir Path testDir,org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testResnetV2(ByVal testDir As Path, ByVal backend As Nd4jBackend)
			If TFGraphTestZooModels.PPC Then
	'            
	'            Ugly hack to temporarily disable tests on PPC only on CI
	'            Issue logged here: https://github.com/eclipse/deeplearning4j/issues/7657
	'            These will be re-enabled for PPC once fixed - in the mean time, remaining tests will be used to detect and prevent regressions
	'             

				log.warn("TEMPORARILY SKIPPING TEST ON PPC ARCHITECTURE DUE TO KNOWN JVM CRASH ISSUES - SEE https://github.com/eclipse/deeplearning4j/issues/7657")
				'OpValidationSuite.ignoreFailing();
			End If

			TFGraphTestZooModels.currentTestDir = testDir.toFile()

			'Load model
			Dim path As String = "tf_graphs/zoo_models/resnetv2_imagenet_frozen_graph/tf_model.txt"
			Dim resource As File = (New ClassPathResource(path)).File
			Dim sd As SameDiff = TFGraphTestZooModels.LOADER.apply(resource, "resnetv2_imagenet_frozen_graph")

			'Load data
			'Because we don't have DataVec NativeImageLoader in ND4J tests due to circular dependencies, we'll load the image previously saved...
			Dim imgFile As File = (New ClassPathResource("deeplearning4j-zoo/goldenretriever_rgb224_unnormalized_nchw_INDArray.bin")).File
			Dim img As INDArray = Nd4j.readBinary(imgFile).castTo(DataType.FLOAT)
			img = img.permute(0,2,3,1).dup() 'to NHWC

			'Resnet v2 - NO external normalization, just resize and center crop
			' https://github.com/tensorflow/models/blob/d32d957a02f5cffb745a4da0d78f8432e2c52fd4/research/tensorrt/tensorrt.py#L70
			' https://github.com/tensorflow/models/blob/1af55e018eebce03fb61bba9959a04672536107d/official/resnet/imagenet_preprocessing.py#L253-L256

			'Perform inference
			Dim inputs As IList(Of String) = sd.inputs()
			assertEquals(1, inputs.Count)

			Dim [out] As String = "softmax_tensor"
			Dim m As IDictionary(Of String, INDArray) = sd.output(Collections.singletonMap(inputs(0), img), [out])

			Dim outArr As INDArray = m([out])


			Console.WriteLine("SHAPE: " & java.util.Arrays.toString(outArr.shape()))
			Console.WriteLine(outArr)

			Dim argmax As INDArray = outArr.argMax(1)

			'Load labels
			Dim labels As IList(Of String) = ValidateZooModelPredictions.labels()

			Dim classIdx As Integer = argmax.getInt(0)
			Dim className As String = labels(classIdx)
			Dim expClass As String = "golden retriever"
			Dim prob As Double = outArr.getDouble(classIdx)

			Console.WriteLine("Predicted class: " & classIdx & " - """ & className & """ - probability = " & prob)
			assertEquals(expClass, className)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static List<String> labels() throws Exception
		Public Shared Function labels() As IList(Of String)
			Dim labelsFile As File = (New ClassPathResource("tf_graphs/zoo_models/labels/imagenet_labellist.txt")).File
'JAVA TO VB CONVERTER NOTE: The local variable labels was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim labels_Conflict As IList(Of String) = FileUtils.readLines(labelsFile, StandardCharsets.UTF_8)
			Return labels_Conflict
		End Function
	End Class

End Namespace