Imports System.Collections.Generic
Imports Getter = lombok.Getter
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
Namespace org.deeplearning4j.rl4j.agent.learning.update


	Public Class FeaturesLabels

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Features features;
		Private ReadOnly features As Features

		Private ReadOnly labels As New Dictionary(Of String, INDArray)()

		''' <param name="features"> </param>
		Public Sub New(ByVal features As Features)
			Me.features = features
		End Sub

		''' <returns> The number of examples in features and each labels. </returns>
		Public Overridable ReadOnly Property BatchSize As Long
			Get
				Return features.getBatchSize()
			End Get
		End Property

		''' <summary>
		''' Add labels by name </summary>
		''' <param name="name"> </param>
		''' <param name="labels"> </param>
		Public Overridable Sub putLabels(ByVal name As String, ByVal labels As INDArray)
			Me.labels(name) = labels
		End Sub

		''' <summary>
		''' Get the labels associated to the name. </summary>
		''' <param name="name">
		''' @return </param>
		Public Overridable Function getLabels(ByVal name As String) As INDArray
			Return Me.labels(name)
		End Function
	End Class

End Namespace