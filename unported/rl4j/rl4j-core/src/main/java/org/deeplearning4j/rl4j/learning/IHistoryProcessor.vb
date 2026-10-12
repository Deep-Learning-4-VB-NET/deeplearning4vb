Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
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

Namespace org.deeplearning4j.rl4j.learning

	Public Interface IHistoryProcessor

		ReadOnly Property Conf As Configuration

		''' <summary>
		''' Returns compressed arrays, which must be rescaled based
		'''  on the value returned by <seealso cref="getScale()"/>. 
		''' </summary>
		ReadOnly Property History As INDArray()

		Sub record(ByVal image As INDArray)

		Sub add(ByVal image As INDArray)

		Sub startMonitor(ByVal filename As String, ByVal shape() As Integer)

		Sub stopMonitor()

		ReadOnly Property Monitoring As Boolean

		''' <summary>
		''' Returns the scale of the arrays returned by <seealso cref="getHistory()"/>, typically 255. </summary>
		ReadOnly Property Scale As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int historyLength = 4;
			Friend historyLength As Integer = 4
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int rescaledWidth = 84;
			Friend rescaledWidth As Integer = 84
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int rescaledHeight = 84;
			Friend rescaledHeight As Integer = 84
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int croppingWidth = 84;
			Friend croppingWidth As Integer = 84
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int croppingHeight = 84;
			Friend croppingHeight As Integer = 84
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int offsetX = 0;
			Friend offsetX As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int offsetY = 0;
			Friend offsetY As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default int skipFrame = 4;
			Friend skipFrame As Integer = 4

			Public Overridable ReadOnly Property Shape As Integer()
				Get
					Return New Integer() {getHistoryLength(), getRescaledHeight(), getRescaledWidth()}
				End Get
			End Property
		End Class
	End Interface

End Namespace