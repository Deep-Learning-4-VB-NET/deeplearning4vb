Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp

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

Namespace org.nd4j.autodiff.samediff.optimize


	''' <summary>
	''' @author Alex Black
	''' </summary>
	Public Interface Optimizer

		''' <param name="sd">              Current SameDiff instance to optimize </param>
		''' <param name="helper">          Helper class for optimization </param>
		''' <param name="op">              Operation to check for optimization </param>
		''' <param name="constantArrays">  Array holder for constant arrays </param>
		''' <param name="variablesArrays"> Array holder for variable arrays </param>
		''' <returns> True if the optimization was applied </returns>
		Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean

	End Interface

End Namespace