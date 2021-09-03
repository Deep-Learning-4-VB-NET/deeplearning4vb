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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class NameScope implements java.io.Closeable
	Public Class NameScope
		Implements System.IDisposable

		Private ReadOnly sameDiff As SameDiff
		Private ReadOnly name As String

'JAVA TO VB CONVERTER NOTE: The parameter sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ByVal sameDiff_Conflict As SameDiff, ByVal name As String)
			Me.sameDiff = sameDiff_Conflict
			Me.name = name
		End Sub

		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			sameDiff.closeNameScope(Me)
		End Sub

		Public Overrides Function ToString() As String
			Return "NameScope(" & name & ")"
		End Function
	End Class

End Namespace