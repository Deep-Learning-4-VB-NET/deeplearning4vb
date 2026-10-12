Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports org.nd4j.linalg.lossfunctions

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
Namespace org.deeplearning4j.gradientcheck.sdlosscustom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) public class SDLossMSE extends SameDiffLoss
	<Serializable>
	Public Class SDLossMSE
		Inherits SameDiffLoss

		Public Overrides Function defineLoss(ByVal sd As SameDiff, ByVal layerInput As SDVariable, ByVal labels As SDVariable) As SDVariable
			Return labels.squaredDifference(layerInput).mean(1)
		End Function
	End Class

End Namespace