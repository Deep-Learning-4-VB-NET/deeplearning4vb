Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer

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

Namespace org.nd4j.autodiff.samediff.optimize.debug

	''' <summary>
	''' Used as a listener for
	''' 
	''' @author Alex Black
	''' </summary>
	Public Interface OptimizationDebugger

		Sub beforeOptimizationCheck(ByVal sd As SameDiff, ByVal op As SameDiffOp, ByVal o As Optimizer)

		Sub afterOptimizationsCheck(ByVal sd As SameDiff, ByVal op As SameDiffOp, ByVal o As Optimizer, ByVal wasApplied As Boolean)

	End Interface

End Namespace