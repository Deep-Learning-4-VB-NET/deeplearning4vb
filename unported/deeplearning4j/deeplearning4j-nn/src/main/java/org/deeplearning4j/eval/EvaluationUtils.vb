Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation

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

Namespace org.deeplearning4j.eval

	<Obsolete>
	Public Class EvaluationUtils
		Inherits org.nd4j.evaluation.EvaluationUtils


		Public Shared Function copyToLegacy(Of T, T1)(ByVal from As IEvaluation(Of T1), ByVal [to] As Type(Of T)) As T
			If from Is Nothing Then
				Return Nothing
			End If
			Preconditions.checkState([to].IsAssignableFrom(from.GetType()), "Invalid classes: %s vs %s", from.GetType(), [to])


			Throw New System.NotSupportedException("Not implemented")
		End Function
	End Class

End Namespace