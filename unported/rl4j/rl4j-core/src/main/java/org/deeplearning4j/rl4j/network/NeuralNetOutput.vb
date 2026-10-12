Imports System.Collections.Generic
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
Namespace org.deeplearning4j.rl4j.network


	Public Class NeuralNetOutput
		Private ReadOnly outputs As New Dictionary(Of String, INDArray)()

		''' <summary>
		''' Store an output with a given key </summary>
		''' <param name="key"> The name of the output </param>
		''' <param name="output"> The output </param>
		Public Overridable Sub put(ByVal key As String, ByVal output As INDArray)
			outputs(key) = output
		End Sub

		''' <param name="key"> The name of the output </param>
		''' <returns> The output associated with the key </returns>
		Public Overridable Function get(ByVal key As String) As INDArray
			Dim result As INDArray = outputs(key)
			If result Is Nothing Then
				Throw New System.ArgumentException(String.Format("There is no element with key '{0}' in the neural net output.", key))
			End If
			Return result
		End Function
	End Class

End Namespace