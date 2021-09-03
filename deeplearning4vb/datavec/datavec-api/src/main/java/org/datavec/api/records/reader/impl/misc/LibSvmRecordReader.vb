﻿Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Configuration = org.datavec.api.conf.Configuration

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

Namespace org.datavec.api.records.reader.impl.misc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LibSvmRecordReader extends SVMLightRecordReader
	<Serializable>
	Public Class LibSvmRecordReader
		Inherits SVMLightRecordReader

		Public Sub New()
			MyBase.New()
		End Sub

		Public Overrides WriteOnly Property Conf As Configuration
			Set(ByVal conf As Configuration)
				MyBase.Conf = conf
			End Set
		End Property
	End Class

End Namespace