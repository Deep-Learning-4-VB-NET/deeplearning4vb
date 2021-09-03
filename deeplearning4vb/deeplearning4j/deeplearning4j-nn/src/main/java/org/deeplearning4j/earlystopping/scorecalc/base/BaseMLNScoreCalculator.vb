Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

Namespace org.deeplearning4j.earlystopping.scorecalc.base

	Public MustInherit Class BaseMLNScoreCalculator
		Inherits BaseScoreCalculator(Of MultiLayerNetwork)


		Protected Friend Sub New(ByVal iterator As DataSetIterator)
			MyBase.New(iterator)
		End Sub

		Protected Friend Overrides Function output(ByVal network As MultiLayerNetwork, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As INDArray
			Return network.output(input, False, fMask, lMask)
		End Function

		Protected Friend Overrides Function scoreMinibatch(ByVal network As MultiLayerNetwork, ByVal features() As INDArray, ByVal labels() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal output() As INDArray) As Double
			Return scoreMinibatch(network, get0(features), get0(labels), get0(fMask), get0(lMask), get0(output))
		End Function

		Protected Friend Overrides Function output(ByVal network As MultiLayerNetwork, ByVal input() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray) As INDArray()
			Return New INDArray(){output(network, get0(input), get0(fMask), get0(lMask))}
		End Function
	End Class

End Namespace