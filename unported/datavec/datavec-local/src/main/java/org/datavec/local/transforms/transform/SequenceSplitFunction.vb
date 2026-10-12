Imports System.Collections.Generic
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports Writable = org.datavec.api.writable.Writable
Imports org.datavec.local.transforms

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

Namespace org.datavec.local.transforms.transform


	Public Class SequenceSplitFunction
		Inherits BaseFlatMapFunctionAdaptee(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))

		Public Sub New(ByVal split As SequenceSplit)
			MyBase.New(New SequenceSplitFunctionAdapter(split))
		End Sub

	End Class

End Namespace