Imports System.Collections.Generic
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports org.deeplearning4j.rl4j.experience
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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
Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning


	Public Class RecurrentNStepQLearningHelper
		Inherits NStepQLearningHelper

		Private ReadOnly actionSpaceSize As Integer

		Public Sub New(ByVal actionSpaceSize As Integer)
			Me.actionSpaceSize = actionSpaceSize
		End Sub

		Public Overrides Function createLabels(ByVal trainingBatchSize As Integer) As INDArray
			Return Nd4j.create(1, actionSpaceSize, trainingBatchSize)
		End Function

		Public Overrides Function getExpectedQValues(ByVal allExpectedQValues As INDArray, ByVal idx As Integer) As INDArray
			Return getElementAtIndex(allExpectedQValues, idx)
		End Function

		Public Overrides Sub setLabels(ByVal labels As INDArray, ByVal idx As Long, ByVal data As INDArray)
			getElementAtIndex(labels, idx).assign(data)
		End Sub

		Public Overrides Function getTargetExpectedQValuesOfLast(ByVal target As IOutputNeuralNet, ByVal trainingBatch As IList(Of StateActionReward(Of Integer)), ByVal features As Features) As INDArray
			Return getElementAtIndex(target.output(features).get(CommonOutputNames.QValues), trainingBatch.Count - 1)
		End Function

		Private Function getElementAtIndex(ByVal array As INDArray, ByVal idx As Long) As INDArray
			Return array.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(idx))
		End Function
	End Class
End Namespace