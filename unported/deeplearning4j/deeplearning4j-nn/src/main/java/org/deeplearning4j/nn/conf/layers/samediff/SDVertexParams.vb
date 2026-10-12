Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.deeplearning4j.nn.conf.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SDVertexParams extends SDLayerParams
	<Serializable>
	Public Class SDVertexParams
		Inherits SDLayerParams

		Protected Friend inputs As IList(Of String)

		''' <summary>
		''' Define the inputs to the DL4J SameDiff Vertex with specific names </summary>
		''' <param name="inputNames"> Names of the inputs. Number here also defines the number of vertex inputs </param>
		''' <seealso cref= #defineInputs(int) </seealso>
		Public Overridable Sub defineInputs(ParamArray ByVal inputNames() As String)
			Preconditions.checkArgument(inputNames IsNot Nothing AndAlso inputNames.Length > 0, "Input names must not be null, and must have length > 0: got %s", inputNames)
			Me.inputs = New List(Of String) From {inputNames}
		End Sub

		''' <summary>
		''' Define the inputs to the DL4J SameDiff vertex with generated names. Names will have format "input_0", "input_1", etc
		''' </summary>
		''' <param name="numInputs"> Number of inputs to the vertex. </param>
		Public Overridable Sub defineInputs(ByVal numInputs As Integer)
			Preconditions.checkArgument(numInputs > 0, "Number of inputs must be > 0: Got %s", numInputs)
			Dim inputNames(numInputs - 1) As String
			For i As Integer = 0 To numInputs - 1
				inputNames(i) = "input_" & i
			Next i
		End Sub

	End Class

End Namespace