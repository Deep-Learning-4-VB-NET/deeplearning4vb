Imports System
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports VariationalAutoencoder = org.deeplearning4j.nn.layers.variational.VariationalAutoencoder
Imports org.deeplearning4j.spark.impl.common.score
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

Namespace org.deeplearning4j.spark.impl.graph.scoring

	Public Class CGVaeReconstructionProbWithKeyFunction(Of K)
		Inherits BaseVaeReconstructionProbWithKeyFunction(Of K)


		''' <param name="params">            MultiLayerNetwork parameters </param>
		''' <param name="jsonConfig">        MultiLayerConfiguration, as json </param>
		''' <param name="useLogProbability"> If true: use log probability. False: use raw probability. </param>
		''' <param name="batchSize">         Batch size to use when scoring </param>
		''' <param name="numSamples">        Number of samples to use when calling <seealso cref="VariationalAutoencoder.reconstructionLogProbability(INDArray, Integer)"/> </param>
		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal useLogProbability As Boolean, ByVal batchSize As Integer, ByVal numSamples As Integer)
			MyBase.New(params, jsonConfig, useLogProbability, batchSize, numSamples)
		End Sub

		Public Overrides ReadOnly Property VaeLayer As VariationalAutoencoder
			Get
				Dim network As New ComputationGraph(ComputationGraphConfiguration.fromJson(CStr(jsonConfig.getValue())))
				network.init()
				Dim val As INDArray = DirectCast(params.value(), INDArray).unsafeDuplication()
				If val.length() <> network.numParams(False) Then
					Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcasted set parameters")
				End If
				network.Params = val
    
				Dim l As Layer = network.getLayer(0)
				If Not (TypeOf l Is VariationalAutoencoder) Then
					Throw New Exception("Cannot use CGVaeReconstructionProbWithKeyFunction on network that doesn't have a VAE " & "layer as layer 0. Layer type: " & l.GetType())
				End If
				Return DirectCast(l, VariationalAutoencoder)
			End Get
		End Property
	End Class

End Namespace