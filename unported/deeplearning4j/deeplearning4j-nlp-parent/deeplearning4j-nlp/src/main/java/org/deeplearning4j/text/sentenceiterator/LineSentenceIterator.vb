Imports System
Imports System.IO
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator

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

Namespace org.deeplearning4j.text.sentenceiterator


	Public Class LineSentenceIterator
		Inherits BaseSentenceIterator

		Private file As Stream
		Private iter As LineIterator
		Private f As File



		Public Sub New(ByVal f As File)
			If Not f.exists() OrElse Not f.isFile() Then
				Throw New System.ArgumentException("Please specify an existing file")
			End If
			Try
				Me.f = f
				Me.file = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				iter = IOUtils.lineIterator(Me.file, "UTF-8")
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Sub

		Public Overrides Function nextSentence() As String
			Dim line As String = iter.nextLine()
			If preProcessor_Conflict IsNot Nothing Then
				line = preProcessor_Conflict.preProcess(line)
			End If
			Return line
		End Function

		Public Overrides Function hasNext() As Boolean
			Return iter.hasNext()
		End Function

		Public Overrides Sub reset()
			Try
				If file IsNot Nothing Then
					file.Close()
				End If
				If iter IsNot Nothing Then
					iter.close()
				End If
				Me.file = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				iter = IOUtils.lineIterator(Me.file, "UTF-8")
			Catch e As IOException
				Throw New Exception(e)
			End Try

		End Sub


	End Class

End Namespace