Imports System.Collections.Generic
Imports FilterOperation = org.deeplearning4j.rl4j.observation.transform.FilterOperation
Imports Preconditions = org.nd4j.common.base.Preconditions

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
Namespace org.deeplearning4j.rl4j.observation.transform.filter


	Public Class UniformSkippingFilter
		Implements FilterOperation

		Private ReadOnly skipFrame As Integer

		''' <param name="skipFrame"> Will cause the filter to keep (not skip) 1 frame every skipFrames. </param>
		Public Sub New(ByVal skipFrame As Integer)
			Preconditions.checkArgument(skipFrame > 0, "skipFrame should be greater than 0")

			Me.skipFrame = skipFrame
		End Sub

		Public Overridable Function isSkipped(ByVal channelsData As IDictionary(Of String, Object), ByVal currentObservationStep As Integer, ByVal isFinalObservation As Boolean) As Boolean Implements FilterOperation.isSkipped
			Return Not isFinalObservation AndAlso (currentObservationStep Mod skipFrame <> 0)
		End Function
	End Class

End Namespace