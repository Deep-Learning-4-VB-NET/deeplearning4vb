﻿Imports System.Collections.Generic
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction

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

Namespace org.deeplearning4j.spark.text.functions


	''' <summary>
	''' @author jeffreytang
	''' </summary>
	Public Class MapPerPartitionVoidFunction
		Implements VoidFunction(Of IEnumerator(Of JavaToDotNetGenericWildcard))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(java.util.Iterator<?> integerIterator) throws Exception
		Public Overrides Sub [call](Of T1)(ByVal integerIterator As IEnumerator(Of T1))
		End Sub
	End Class


End Namespace