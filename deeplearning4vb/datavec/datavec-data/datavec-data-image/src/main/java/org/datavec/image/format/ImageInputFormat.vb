﻿Imports System
Imports Configuration = org.datavec.api.conf.Configuration
Imports BaseInputFormat = org.datavec.api.formats.input.BaseInputFormat
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader

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

Namespace org.datavec.image.format


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class ImageInputFormat
		Inherits BaseInputFormat

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.reader.RecordReader createReader(org.datavec.api.split.InputSplit split, org.datavec.api.conf.Configuration conf) throws IOException, InterruptedException
		Public Overrides Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader
			Dim reader As RecordReader = New ImageRecordReader()
			reader.initialize(conf, split)
			Return reader
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.reader.RecordReader createReader(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Function createReader(ByVal split As InputSplit) As RecordReader
			Return createReader(split, New Configuration())
		End Function

	End Class

End Namespace