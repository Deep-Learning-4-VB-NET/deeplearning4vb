Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient

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


	Public Class Gradients

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final long batchSize;
		Private ReadOnly batchSize As Long

		Private ReadOnly gradients As New Dictionary(Of String, Gradient)()

		''' <param name="batchSize"> The size of the training batch used to create this instance </param>
		Public Sub New(ByVal batchSize As Long)
			Me.batchSize = batchSize
		End Sub

		''' <summary>
		''' Add a <seealso cref="Gradient"/> by name. </summary>
		''' <param name="name"> </param>
		''' <param name="gradient"> </param>
		Public Overridable Sub putGradient(ByVal name As String, ByVal gradient As Gradient)
			gradients(name) = gradient
		End Sub

		''' <summary>
		''' Get a <seealso cref="Gradient"/> by name </summary>
		''' <param name="name">
		''' @return </param>
		Public Overridable Function getGradient(ByVal name As String) As Gradient
			Return gradients(name)
		End Function

	End Class

End Namespace