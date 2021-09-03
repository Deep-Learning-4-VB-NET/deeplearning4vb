﻿Imports Configuration = org.datavec.api.conf.Configuration
Imports DataVecException = org.datavec.api.exceptions.DataVecException
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

Namespace org.datavec.api.formats.output


	Public Interface OutputFormat

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		public static final String OUTPUT_PATH = "org.nd4j.outputpath";

		''' <summary>
		''' Create a record writer </summary>
		''' <returns> the created writer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.datavec.api.records.writer.RecordWriter createWriter(org.datavec.api.conf.Configuration conf) throws org.datavec.api.exceptions.DataVecException;
		Function createWriter(ByVal conf As Configuration) As RecordWriter

	End Interface

End Namespace