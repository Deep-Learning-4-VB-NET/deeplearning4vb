﻿Imports Function2 = org.apache.spark.api.java.function.Function2
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

Namespace org.datavec.spark.transform.utils.adapter

	Public Class BiFunctionAdapter(Of A, B, R)
		Implements Function2(Of A, B, R)

		Private ReadOnly fn As BiFunction(Of A, B, R)

		Public Sub New(ByVal fn As BiFunction(Of A, B, R))
			Me.fn = fn
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public R call(A v1, B v2) throws Exception
		Public Overrides Function [call](ByVal v1 As A, ByVal v2 As B) As R
			Return fn.apply(v1, v2)
		End Function
	End Class

End Namespace