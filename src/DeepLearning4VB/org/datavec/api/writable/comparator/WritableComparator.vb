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

Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.writable.comparator

    ''' <summary>
    ''' A serializable comparer of writables. The Java <c>@JsonTypeInfo(use = CLASS, property = "@class")</c>
    ''' annotation is mapped to System.Text.Json polymorphism with the Java class names as discriminators.
    ''' </summary>
    <JsonPolymorphic(TypeDiscriminatorPropertyName:="@class", UnknownDerivedTypeHandling:=JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)>
    <JsonDerivedType(GetType(DoubleWritableComparator), "org.datavec.api.writable.comparator.DoubleWritableComparator")>
    <JsonDerivedType(GetType(FloatWritableComparator), "org.datavec.api.writable.comparator.FloatWritableComparator")>
    <JsonDerivedType(GetType(IntWritableComparator), "org.datavec.api.writable.comparator.IntWritableComparator")>
    <JsonDerivedType(GetType(LongWritableComparator), "org.datavec.api.writable.comparator.LongWritableComparator")>
    <JsonDerivedType(GetType(TextWritableComparator), "org.datavec.api.writable.comparator.TextWritableComparator")>
    Public Interface WritableComparator
        Inherits IComparer(Of Writable)

    End Interface

End Namespace
