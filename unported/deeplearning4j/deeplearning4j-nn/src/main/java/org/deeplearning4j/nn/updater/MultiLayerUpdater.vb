Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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

Namespace org.deeplearning4j.nn.updater


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Slf4j public class MultiLayerUpdater extends BaseMultiLayerUpdater<org.deeplearning4j.nn.multilayer.MultiLayerNetwork>
	<Serializable>
	Public Class MultiLayerUpdater
		Inherits BaseMultiLayerUpdater(Of MultiLayerNetwork)

		Public Sub New(ByVal network As MultiLayerNetwork)
			Me.New(network, Nothing)
		End Sub

		Public Sub New(ByVal network As MultiLayerNetwork, ByVal updaterState As INDArray)
			MyBase.New(network, updaterState)

			layersByName = New Dictionary(Of String, Trainable)()
			Dim l() As Layer = network.Layers
			For i As Integer = 0 To l.Length - 1
				layersByName(i.ToString()) = l(i)
			Next i
		End Sub

		Protected Friend Overrides ReadOnly Property OrderedLayers As Trainable()
			Get
				Dim layers() As Layer = network.getLayers()
				Dim t(layers.Length - 1) As Trainable
				Array.Copy(layers, 0, t, 0, layers.Length)
				Return t
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property FlattenedGradientsView As INDArray
			Get
				If network.getFlattenedGradients() Is Nothing Then
					network.initGradientsView()
				End If
				Return network.getFlattenedGradients()
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property Params As INDArray
			Get
				Return network.params()
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property MiniBatch As Boolean
			Get
				Return network.conf().isMiniBatch()
			End Get
		End Property

		Public Overrides Function clone() As Updater
			Return New MultiLayerUpdater(network, Nothing)
		End Function
	End Class

End Namespace