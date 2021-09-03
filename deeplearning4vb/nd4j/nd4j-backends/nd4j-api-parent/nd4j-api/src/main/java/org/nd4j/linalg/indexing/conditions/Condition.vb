﻿Imports org.nd4j.common.function

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

Namespace org.nd4j.linalg.indexing.conditions

	''' 
	''' <summary>
	''' Sets a condition in correspondence with the MatchConditionalBool op
	''' (op num 5 in the legacy operations)
	''' 
	''' Condition number is affected by the ops internals, see here for the comprehensive overview:
	''' https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/ops/ops.h#L2253
	''' 
	''' As of this writing (July 27,2021), the following operations are relevant:
	''' 0: equals
	''' 1: not equals
	''' 2: less than
	''' 3: greater than
	''' 4: less than or equal
	''' 5: greater than or equal
	''' 6: absolute difference less than
	''' 7: absolute difference greater than
	''' 8: is infinite
	''' 9: is nan
	''' 10: absolute equals
	''' 11: not equals
	''' 12: absolute difference greater or equal to
	''' 13: absolute difference less than or equal to
	''' 14: is finite
	''' 15: is infinite
	''' 
	''' @return
	''' </summary>
	Public Interface Condition
		Inherits [Function](Of Number, Boolean)

		''' <summary>
		''' Allows overriding of the value.
		''' </summary>
		''' <param name="value"> </param>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java default interface methods:
'		default void setValue(Number value)
	'	{
	'		'no-op for aggregate conditions. Mainly used for providing an api to end users such as:
	'		'INDArray.match(input,Conditions.equals())
	'		'See: https://github.com/eclipse/deeplearning4j/issues/9393
	'	}

		''' <summary>
		''' Returns condition ID for native side
		''' 
		''' @return
		''' </summary>
		Function condtionNum() As Integer

		ReadOnly Property Value As Double

		Function epsThreshold() As Double

		Overrides Function apply(ByVal input As Number) As Boolean
	End Interface

End Namespace