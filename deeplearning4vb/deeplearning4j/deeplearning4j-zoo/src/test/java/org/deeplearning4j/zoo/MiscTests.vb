Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports VGG16 = org.deeplearning4j.zoo.model.VGG16
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.zoo

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class MiscTests extends org.deeplearning4j.BaseDL4JTest
	Public Class MiscTests
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransferVGG() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTransferVGG()
			Dim ds As New DataSet()
			ds.Features = Nd4j.create(1, 3, 224, 224)
			ds.Labels = Nd4j.create(1, 2)

			Dim model As ComputationGraph = CType(VGG16.builder().build().initPretrained(PretrainedType.IMAGENET), ComputationGraph)
	'        System.out.println(model.summary());

			Dim transferModel As ComputationGraph = (New TransferLearning.GraphBuilder(model)).setFeatureExtractor("fc2").removeVertexKeepConnections("predictions").addLayer("predictions", (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nIn(4096).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build(), "fc2").build()

	'        System.out.println(transferModel.summary());
	'        System.out.println("Fitting");
			transferModel.fit(ds)

			Dim g2 As ComputationGraph = TestUtils.testModelSerialization(transferModel)
			g2.fit(ds)
		End Sub

	End Class

End Namespace