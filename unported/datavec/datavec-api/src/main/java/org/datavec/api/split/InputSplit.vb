Imports System.Collections.Generic
Imports System.IO

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



	Public Interface InputSplit


		''' <summary>
		''' Returns true if the given uri
		''' can be written to </summary>
		''' <param name="location"> the location to determine
		''' @return </param>
		Function canWriteToLocation(ByVal location As URI) As Boolean

		''' <summary>
		''' Add a new location with the name generated
		'''  by this input split/
		''' </summary>
		Function addNewLocation() As String

		''' <summary>
		''' Add a new location to this input split
		''' (this  may do anything from updating an in memory location
		''' to creating a new file) </summary>
		''' <param name="location"> the location to add </param>
		Function addNewLocation(ByVal location As String) As String

		''' <summary>
		''' Refreshes the split locations
		''' if needed in memory.
		''' (Think a few file gets added) </summary>
		''' <param name="reset"> </param>
		Sub updateSplitLocations(ByVal reset As Boolean)


		''' <summary>
		''' Returns true if this <seealso cref="InputSplit"/>
		''' needs bootstrapping for writing.
		''' A simple example of needing bootstrapping is for
		''' <seealso cref="FileSplit"/> where there is only a directory
		''' existing, but no file to write to </summary>
		''' <returns> true if this input split needs bootstrapping for
		''' writing to or not </returns>
		Function needsBootstrapForWrite() As Boolean

		''' <summary>
		''' Bootstrap this input split for writing.
		''' This is for use with <seealso cref="org.datavec.api.records.writer.RecordWriter"/>
		''' </summary>
		Sub bootStrapForWrite()

		''' <summary>
		''' Open an <seealso cref="System.IO.Stream_Output"/>
		''' for the given location.
		''' Note that the user is responsible for closing
		''' the associated output stream. </summary>
		''' <param name="location"> the location to open the output stream for </param>
		''' <returns> the output input stream </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.io.OutputStream openOutputStreamFor(String location) throws Exception;
		Function openOutputStreamFor(ByVal location As String) As Stream

		''' <summary>
		''' Open an <seealso cref="System.IO.Stream_Input"/>
		''' for the given location.
		''' Note that the user is responsible for closing
		''' the associated input stream. </summary>
		''' <param name="location"> the location to open the input stream for </param>
		''' <returns> the opened input stream </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.io.InputStream openInputStreamFor(String location) throws Exception;
		Function openInputStreamFor(ByVal location As String) As Stream

		''' <summary>
		'''  Length of the split
		''' @return
		''' </summary>
		Function length() As Long

		''' <summary>
		''' Locations of the splits
		''' @return
		''' </summary>
		Function locations() As URI()

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function locationsIterator() As IEnumerator(Of URI)

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function locationsPathIterator() As IEnumerator(Of String)

		''' <summary>
		''' Reset the InputSplit without reinitializing it from scratch.
		''' In many cases, this is a no-op.
		''' For InputSplits that have randomization: reset should shuffle the order.
		''' </summary>
		Sub reset()

		''' <returns> True if the reset() method is supported (or is a no-op), false otherwise. If false is returned, reset()
		'''         may throw an exception </returns>
		Function resetSupported() As Boolean
	End Interface

End Namespace