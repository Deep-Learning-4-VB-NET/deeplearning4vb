﻿Imports System
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

Namespace org.datavec.api.writable.comparator

	<Serializable>
	Public Class IntWritableComparator
		Implements WritableComparator

		Public Overridable Function Compare(ByVal o1 As Writable, ByVal o2 As Writable) As Integer
			Return Integer.compare(o1.toInt(), o2.toInt())
		End Function

		Public Overrides Function ToString() As String
			Return "IntWritableComparator()"
		End Function
	End Class

End Namespace