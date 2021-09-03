Imports System.Collections.Generic
Imports Join = org.datavec.api.transform.join.Join
Imports Writable = org.datavec.api.writable.Writable
Imports org.datavec.local.transforms
Imports org.nd4j.common.primitives

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

Namespace org.datavec.local.transforms.join


	Public Class ExecuteJoinFromCoGroupFlatMapFunction
		Inherits BaseFlatMapFunctionAdaptee(Of Pair(Of IList(Of Writable), Pair(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))), IList(Of Writable))

		Public Sub New(ByVal join As Join)
			MyBase.New(New ExecuteJoinFromCoGroupFlatMapFunctionAdapter(join))
		End Sub
	End Class

End Namespace