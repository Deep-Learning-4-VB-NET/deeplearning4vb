Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter

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

Namespace org.datavec.api.records.listener


	Public Interface RecordListener
		''' <summary>
		''' Get if listener invoked.
		''' </summary>
		Function invoked() As Boolean

		''' <summary>
		''' Change invoke to true.
		''' </summary>
		Sub invoke()

		''' <summary>
		''' Event listener for each record to be read. </summary>
		''' <param name="reader"> the record reader </param>
		''' <param name="record"> in raw format (Collection, File, String, Writable, etc) </param>
		Sub recordRead(ByVal reader As RecordReader, ByVal record As Object)

		''' <summary>
		''' Event listener for each record to be written. </summary>
		''' <param name="writer"> the record writer </param>
		''' <param name="record"> in raw format (Collection, File, String, Writable, etc) </param>
		Sub recordWrite(ByVal writer As RecordWriter, ByVal record As Object)
	End Interface

End Namespace