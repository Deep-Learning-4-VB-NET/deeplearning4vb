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

Namespace org.datavec.image.recordreader.objdetect

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ImageObject
	Public Class ImageObject

		Private ReadOnly x1 As Integer
		Private ReadOnly y1 As Integer
		Private ReadOnly x2 As Integer
		Private ReadOnly y2 As Integer
		Private ReadOnly label As String

		Public Sub New(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal label As String)
			If x1 > x2 OrElse y1 > y2 Then
				Throw New System.ArgumentException("Invalid input: (x1,y1), top left position must have values less than" & " (x2,y2) bottom right position. Got: (" & x1 & "," & y1 & "), (" & x2 & "," & y2 & ")")
			End If

			Me.x1 = x1
			Me.y1 = y1
			Me.x2 = x2
			Me.y2 = y2
			Me.label = label
		End Sub

		Public Overridable ReadOnly Property XCenterPixels As Double
			Get
				Return (x1 + x2) / 2.0
			End Get
		End Property

		Public Overridable ReadOnly Property YCenterPixels As Double
			Get
				Return (y1 + y2) / 2.0
			End Get
		End Property
	End Class

End Namespace