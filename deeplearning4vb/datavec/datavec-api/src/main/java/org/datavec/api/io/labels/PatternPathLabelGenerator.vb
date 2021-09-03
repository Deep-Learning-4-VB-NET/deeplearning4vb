Imports System
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.io.labels


	''' <summary>
	''' Returns a label derived from the base name of the path. Splits the base name
	''' of the path with the given regex pattern, and returns the patternPosition'th
	''' element of the array.
	''' 
	''' @author saudet
	''' </summary>
	<Serializable>
	Public Class PatternPathLabelGenerator
		Implements PathLabelGenerator

		Protected Friend pattern As String ' Pattern to split and segment file name, pass in regex
		Protected Friend patternPosition As Integer = 0

		Public Sub New(ByVal pattern As String)
			Me.pattern = pattern
		End Sub

		Public Sub New(ByVal pattern As String, ByVal patternPosition As Integer)
			Me.pattern = pattern
			Me.patternPosition = patternPosition
		End Sub

		Public Overridable Function getLabelForPath(ByVal path As String) As Writable Implements PathLabelGenerator.getLabelForPath
			' Label is in the filename
			Return New Text(FilenameUtils.getBaseName(path).Split(pattern)(patternPosition))
		End Function

		Public Overridable Function getLabelForPath(ByVal uri As URI) As Writable Implements PathLabelGenerator.getLabelForPath
			Return getLabelForPath((New File(uri)).ToString())
		End Function

		Public Overridable Function inferLabelClasses() As Boolean Implements PathLabelGenerator.inferLabelClasses
			Return True
		End Function
	End Class

End Namespace