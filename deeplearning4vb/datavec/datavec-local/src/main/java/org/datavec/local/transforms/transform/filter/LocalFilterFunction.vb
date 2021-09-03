﻿Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Filter = org.datavec.api.transform.filter.Filter
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function

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

Namespace org.datavec.local.transforms.transform.filter


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class LocalFilterFunction implements org.nd4j.common.function.@Function<java.util.List<org.datavec.api.writable.Writable>, Boolean>
	Public Class LocalFilterFunction
		Implements [Function](Of IList(Of Writable), Boolean)

		Private ReadOnly filter As Filter

		Public Overridable Function apply(ByVal v1 As IList(Of Writable)) As Boolean?
			Return Not filter.removeExample(v1) 'return true to keep example (Filter: return true to remove)
		End Function
	End Class

End Namespace