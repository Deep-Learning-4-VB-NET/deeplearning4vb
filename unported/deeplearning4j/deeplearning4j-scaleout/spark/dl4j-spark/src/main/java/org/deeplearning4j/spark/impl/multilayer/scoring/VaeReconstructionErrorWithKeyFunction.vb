Imports System
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports VariationalAutoencoder = org.deeplearning4j.nn.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.spark.impl.common.score
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.impl.multilayer.scoring



	Public Class VaeReconstructionErrorWithKeyFunction(Of K)
		Inherits BaseVaeScoreWithKeyFunction(Of K)

		''' <param name="params">            MultiLayerNetwork parameters </param>
		''' <param name="jsonConfig">        MultiLayerConfiguration, as json </param>
		''' <param name="batchSize">         Batch size to use when scoring </param>
		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal batchSize As Integer)
			MyBase.New(params, jsonConfig, batchSize)
		End Sub

		Public Overrides ReadOnly Property VaeLayer As VariationalAutoencoder
			Get
				Dim network As New MultiLayerNetwork(MultiLayerConfiguration.fromJson(CStr(jsonConfig.getValue())))
				network.init()
				Dim val As INDArray = DirectCast(params.value(), INDArray).unsafeDuplication()
				If val.length() <> network.numParams(False) Then
					Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcast set parameters")
				End If
				network.Parameters = val
    
				Dim l As Layer = network.getLayer(0)
				If Not (TypeOf l Is VariationalAutoencoder) Then
					Throw New Exception("Cannot use VaeReconstructionErrorWithKeyFunction on network that doesn't have a VAE " & "layer as layer 0. Layer type: " & l.GetType())
				End If
				Return DirectCast(l, VariationalAutoencoder)
			End Get
		End Property

		Public Overrides Function computeScore(ByVal vae As VariationalAutoencoder, ByVal toScore As INDArray) As INDArray
			Return vae.reconstructionError(toScore)
		End Function
	End Class

End Namespace