Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data

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

Namespace org.nd4j.autodiff.listeners.checkpoint


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class Checkpoint implements java.io.Serializable
	<Serializable>
	Public Class Checkpoint

		Private checkpointNum As Integer
		Private timestamp As Long
		Private iteration As Integer
		Private epoch As Integer
		Private filename As String

		Public Shared ReadOnly Property FileHeader As String
			Get
				Return "checkpointNum,timestamp,iteration,epoch,filename"
			End Get
		End Property

		Public Shared Function fromFileString(ByVal str As String) As Checkpoint
			Dim split() As String = str.Split(",", True)
			If split.Length <> 5 Then
				Throw New System.InvalidOperationException("Cannot parse checkpoint entry: expected 5 entries, got " & split.Length & " - values = " & Arrays.toString(split))
			End If
			Return New Checkpoint(Integer.Parse(split(0)), Long.Parse(split(1)), Integer.Parse(split(2)), Integer.Parse(split(3)), split(4))
		End Function

		Public Overridable Function toFileString() As String
			Return checkpointNum & "," & timestamp & "," & iteration & "," & epoch & "," & filename
		End Function
	End Class

End Namespace