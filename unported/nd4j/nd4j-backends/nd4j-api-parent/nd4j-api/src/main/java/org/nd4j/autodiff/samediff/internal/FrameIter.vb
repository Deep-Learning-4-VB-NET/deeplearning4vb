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

Namespace org.nd4j.autodiff.samediff.internal

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class FrameIter
	Public Class FrameIter
		Private frame As String
		Private iteration As Integer
		Private parentFrame As FrameIter

		Public Sub New(ByVal frame As String, ByVal iteration As Integer, ByVal parentFrame As FrameIter)
			Me.frame = frame
			Me.iteration = iteration
			Me.parentFrame = parentFrame
		End Sub

		Public Overrides Function ToString() As String
			Return "(""" & frame & """," & iteration + (If(parentFrame Is Nothing, "", ",parent=" & parentFrame.ToString())) & ")"
		End Function

		Public Overrides Function clone() As FrameIter
			Return New FrameIter(frame, iteration, (If(parentFrame Is Nothing, Nothing, parentFrame.clone())))
		End Function

		Public Overridable Function toVarId(ByVal name As String) As AbstractSession.VarId
			Return New AbstractSession.VarId(name, frame, iteration, parentFrame)
		End Function
	End Class

End Namespace