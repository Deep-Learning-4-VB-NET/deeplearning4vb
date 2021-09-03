Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
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

Namespace org.datavec.api.records.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class Record implements org.datavec.api.records.Record
	Public Class Record
		Implements org.datavec.api.records.Record

		Private record As IList(Of Writable)
		Private metaData As RecordMetaData

	End Class

End Namespace