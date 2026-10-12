Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

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

Namespace org.deeplearning4j.util

	Public Class CapsuleUtils

		''' <summary>
		'''  Compute the squash operation used in CapsNet
		'''  The formula is (||s||^2 / (1 + ||s||^2)) * (s / ||s||).
		'''  Canceling one ||s|| gives ||s||*s/((1 + ||s||^2)
		''' </summary>
		''' <param name="SD"> The SameDiff environment </param>
		''' <param name="x"> The variable to squash </param>
		''' <returns> squash(x) </returns>
		Public Shared Function squash(ByVal SD As SameDiff, ByVal x As SDVariable, ByVal [dim] As Integer) As SDVariable
			Dim squaredNorm As SDVariable = SD.math_Conflict.square(x).sum(True, [dim])
			Dim scale As SDVariable = SD.math_Conflict.sqrt(squaredNorm.plus(1e-5))
			Return x.times(squaredNorm).div(squaredNorm.plus(1.0).times(scale))
		End Function

	End Class

End Namespace