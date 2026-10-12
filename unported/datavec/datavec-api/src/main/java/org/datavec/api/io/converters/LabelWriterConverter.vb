Imports System.Collections.Generic
Imports WritableConverter = org.datavec.api.io.WritableConverter
Imports IntWritable = org.datavec.api.writable.IntWritable
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

Namespace org.datavec.api.io.converters


	Public Class LabelWriterConverter
		Implements WritableConverter

		Private labels As IList(Of String)

		Public Sub New(ByVal labels As IList(Of String))
			Me.labels = labels
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.writable.Writable convert(org.datavec.api.writable.Writable writable) throws WritableConverterException
		Public Overridable Function convert(ByVal writable As Writable) As Writable Implements WritableConverter.convert
			Dim label As String = writable.ToString()
			Return New IntWritable(labels.IndexOf(label))
		End Function
	End Class

End Namespace