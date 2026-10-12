Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports UriFromPathIterator = org.datavec.api.util.files.UriFromPathIterator

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

Namespace org.datavec.api.split


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NumberedFileInputSplit implements InputSplit
	Public Class NumberedFileInputSplit
		Implements InputSplit

		Private ReadOnly baseString As String
		Private ReadOnly minIdx As Integer
		Private ReadOnly maxIdx As Integer

		Private Shared ReadOnly p As Pattern = Pattern.compile("\%(0\d)?d")

		''' <param name="baseString"> String that defines file format. Must contain "%d", which will be replaced with
		'''                   the index of the file, possibly zero-padded to x digits if the pattern is in the form %0xd. </param>
		''' <param name="minIdxInclusive"> Minimum index/number (starting number in sequence of files, inclusive) </param>
		''' <param name="maxIdxInclusive"> Maximum index/number (last number in sequence of files, inclusive) </param>
		'''                        <seealso cref= {NumberedFileInputSplitTest} </seealso>
		Public Sub New(ByVal baseString As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer)
			Dim m As Matcher = p.matcher(baseString)
			If baseString Is Nothing OrElse Not m.find() Then
				Throw New System.ArgumentException("Base String must match this regular expression: " & p.ToString())
			End If
			Me.baseString = baseString
			Me.minIdx = minIdxInclusive
			Me.maxIdx = maxIdxInclusive
		End Sub

		Public Overridable Function canWriteToLocation(ByVal location As URI) As Boolean Implements InputSplit.canWriteToLocation
			Return location.isAbsolute()
		End Function

		Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
			Return Nothing
		End Function

		Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
			Return Nothing
		End Function

		Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
			'no-op (locations() is dynamic)
		End Sub

		Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
			Return locations() Is Nothing OrElse locations().Length < 1 OrElse locations().Length = 1 AndAlso Not locations()(0).isAbsolute()
		End Function

		Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
			If locations().Length = 1 AndAlso Not locations()(0).isAbsolute() Then
				Dim parentDir As New File(locations()(0))
				Dim writeFile As New File(parentDir,"write-file")
				Try
					writeFile.createNewFile()
				Catch e As IOException
					log.error("",e)
				End Try


			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public OutputStream openOutputStreamFor(String location) throws Exception
		Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream
			Dim ret As FileStream = If(location.StartsWith("file:", StringComparison.Ordinal), New FileStream(URI.create(location), FileMode.Create, FileAccess.Write), New FileStream(location, FileMode.Create, FileAccess.Write))
			Return ret
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public InputStream openInputStreamFor(String location) throws Exception
		Public Overridable Function openInputStreamFor(ByVal location As String) As Stream
			Dim fileInputStream As New FileStream(location, FileMode.Open, FileAccess.Read)
			Return fileInputStream
		End Function

		Public Overridable Function length() As Long Implements InputSplit.length
			Return maxIdx - minIdx + 1
		End Function

		Public Overridable Function locations() As URI() Implements InputSplit.locations
			Dim uris(CInt(length()) - 1) As URI
			Dim x As Integer = 0
			If baseString.matches(".{2,}:/.*") Then
				'URI (has scheme)
				For i As Integer = minIdx To maxIdx
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: uris[x++] = java.net.URI.create(String.format(baseString, i));
					uris(x) = URI.create(String.format(baseString, i))
						x += 1
				Next i
			Else
				'File, no scheme
				For i As Integer = minIdx To maxIdx
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: uris[x++] = new File(String.format(baseString, i)).toURI();
					uris(x) = (New File(String.format(baseString, i))).toURI()
						x += 1
				Next i
			End If
			Return uris
		End Function

		Public Overridable Function locationsIterator() As IEnumerator(Of URI) Implements InputSplit.locationsIterator
			Return New UriFromPathIterator(locationsPathIterator())
		End Function

		Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
			Return New NumberedFileIterator(Me)
		End Function

		Public Overridable Sub reset() Implements InputSplit.reset
			'No op
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
			Return True
		End Function


		Private Class NumberedFileIterator
			Implements IEnumerator(Of String)

			Private ReadOnly outerInstance As NumberedFileInputSplit


			Friend currIdx As Integer

			Friend Sub New(ByVal outerInstance As NumberedFileInputSplit)
				Me.outerInstance = outerInstance
				currIdx = outerInstance.minIdx
			End Sub

			Public Overrides Function hasNext() As Boolean
				Return currIdx <= outerInstance.maxIdx
			End Function

			Public Overrides Function [next]() As String
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not hasNext() Then
					Throw New NoSuchElementException()
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return String.format(baseString, currIdx++);
				Dim tempVar = String.format(outerInstance.baseString, currIdx)
					currIdx += 1
					Return tempVar
			End Function

			Public Overrides Sub remove()
				Throw New System.NotSupportedException()
			End Sub
		End Class
	End Class

End Namespace