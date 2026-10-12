Imports System
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.datavec.api.records.listener.impl

	<Serializable>
	Public Class LogRecordListener
		Implements RecordListener

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(LogRecordListener))
'JAVA TO VB CONVERTER NOTE: The field invoked was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private invoked_Conflict As Boolean = False

		Public Overridable Function invoked() As Boolean Implements RecordListener.invoked
			Return invoked_Conflict
		End Function

		Public Overridable Sub invoke() Implements RecordListener.invoke
			Me.invoked_Conflict = True
		End Sub

		Public Overridable Sub recordRead(ByVal reader As RecordReader, ByVal record As Object) Implements RecordListener.recordRead
			invoke()
			log.info("Reading " & record)
		End Sub

		Public Overridable Sub recordWrite(ByVal writer As RecordWriter, ByVal record As Object) Implements RecordListener.recordWrite
			invoke()
			log.info("Writing " & record)
		End Sub
	End Class

End Namespace