Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports AutoEncoder = org.deeplearning4j.nn.conf.layers.AutoEncoder
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Seed Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) @Tag(TagNames.RNG) class SeedTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class SeedTest
		Inherits BaseDL4JTest

		Private irisIter As DataSetIterator = New IrisDataSetIterator(50, 50)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Private data As DataSet = irisIter.next()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Auto Encoder Seed") void testAutoEncoderSeed()
		Friend Overridable Sub testAutoEncoderSeed()
			Dim layerType As AutoEncoder = (New AutoEncoder.Builder()).nIn(4).nOut(3).corruptionLevel(0.0).activation(Activation.SIGMOID).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(layerType).seed(123).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			layer.BackpropGradientsViewArray = Nd4j.create(1, numParams)
			layer.fit(data.Features, LayerWorkspaceMgr.noWorkspaces())
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			Dim score As Double = layer.score()
			Dim parameters As INDArray = layer.params()
			layer.Params = parameters
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			Dim score2 As Double = layer.score()
			assertEquals(parameters, layer.params())
			assertEquals(score, score2, 1e-4)
		End Sub
	End Class

End Namespace