Imports System
Imports System.IO
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


	<Serializable>
	Public Class ParentPathLabelGenerator
		Implements PathLabelGenerator

		Public Sub New()
		End Sub

		Public Overridable Function getLabelForPath(ByVal path As String) As Writable Implements PathLabelGenerator.getLabelForPath
			' Label is in the directory
			Dim dirName As String = FilenameUtils.getName(Path.GetDirectoryName(path))
			Return New Text(dirName)
		End Function

		Public Overridable Function getLabelForPath(ByVal uri As URI) As Writable Implements PathLabelGenerator.getLabelForPath
			Return getLabelForPath((New File(uri)).ToString())
		End Function

		Public Overridable Function inferLabelClasses() As Boolean Implements PathLabelGenerator.inferLabelClasses
			Return True
		End Function
	End Class

End Namespace