Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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
Namespace org.nd4j.autodiff.listeners.profiler.data


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data @AllArgsConstructor @NoArgsConstructor public class TraceEvent
	Public Class TraceEvent

		Private name As String 'Name of event (usually op name)
		Private categories As IList(Of String) 'Comma separated list of categories
		Private ph As Phase 'Event type - phase (see table for options)
		Private ts As Long 'Timestamp, in microseconds (us)
		Private dur As Long? 'Duration, optional
		Private tts As Long? 'Optional, thlread timestamp, in microseconds
		Private pid As Long 'Process ID
		Private tid As Long 'Thread ID
		Private args As IDictionary(Of String, Object) 'Args
		Private cname As ColorName 'Optional, color name (must be one of reserved color names: https://github.com/catapult-project/catapult/blob/master/tracing/tracing/base/color_scheme.html )

	End Class

End Namespace