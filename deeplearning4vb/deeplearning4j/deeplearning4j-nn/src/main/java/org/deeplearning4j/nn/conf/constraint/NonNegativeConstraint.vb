Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.deeplearning4j.nn.conf.constraint

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class NonNegativeConstraint extends BaseConstraint
	<Serializable>
	Public Class NonNegativeConstraint
		Inherits BaseConstraint

		Public Sub New()
		End Sub

		Public Overrides Sub apply(ByVal param As INDArray)
			BooleanIndexing.replaceWhere(param, 0.0, Conditions.lessThan(0.0))
		End Sub

		Public Overrides Function clone() As NonNegativeConstraint
			Return New NonNegativeConstraint()
		End Function

	End Class

End Namespace