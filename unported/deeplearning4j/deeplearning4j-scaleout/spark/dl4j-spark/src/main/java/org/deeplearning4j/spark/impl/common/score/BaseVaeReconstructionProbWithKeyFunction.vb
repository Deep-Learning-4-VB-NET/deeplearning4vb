Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VariationalAutoencoder = org.deeplearning4j.nn.layers.variational.VariationalAutoencoder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.spark.impl.common.score

	Public MustInherit Class BaseVaeReconstructionProbWithKeyFunction(Of K)
		Inherits BaseVaeScoreWithKeyFunction(Of K)

		Private ReadOnly useLogProbability As Boolean
		Private ReadOnly numSamples As Integer

		''' <param name="params">                 MultiLayerNetwork parameters </param>
		''' <param name="jsonConfig">             MultiLayerConfiguration, as json </param>
		''' <param name="useLogProbability">      If true: use log probability. False: use raw probability. </param>
		''' <param name="batchSize">              Batch size to use when scoring </param>
		''' <param name="numSamples">             Number of samples to use when calling <seealso cref="VariationalAutoencoder.reconstructionLogProbability(INDArray, Integer)"/> </param>
		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal useLogProbability As Boolean, ByVal batchSize As Integer, ByVal numSamples As Integer)
			MyBase.New(params, jsonConfig, batchSize)
			Me.useLogProbability = useLogProbability
			Me.numSamples = numSamples
		End Sub

		Public Overrides Function computeScore(ByVal vae As VariationalAutoencoder, ByVal toScore As INDArray) As INDArray
			If useLogProbability Then
				Return vae.reconstructionLogProbability(toScore, numSamples)
			Else
				Return vae.reconstructionProbability(toScore, numSamples)
			End If
		End Function
	End Class

End Namespace