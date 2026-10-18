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

Imports System
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform.serde

Namespace org.datavec.api.writable

    ''' <summary>
    ''' A value that can be serialized to and from a Hadoop-style binary stream. The Java
    ''' <c>@JsonTypeInfo(use = CLASS, property = "@class")</c> annotation is mapped to System.Text.Json
    ''' polymorphism with the Java class names as discriminators, so JSON written by DataVec can be read.
    ''' </summary>
    <JsonClassType>
    <JsonPolymorphic(TypeDiscriminatorPropertyName:="@class", UnknownDerivedTypeHandling:=JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)>
    <JsonDerivedType(GetType(BooleanWritable), "org.datavec.api.writable.BooleanWritable")>
    <JsonDerivedType(GetType(ByteWritable), "org.datavec.api.writable.ByteWritable")>
    <JsonDerivedType(GetType(BytesWritable), "org.datavec.api.writable.BytesWritable")>
    <JsonDerivedType(GetType(DoubleWritable), "org.datavec.api.writable.DoubleWritable")>
    <JsonDerivedType(GetType(FloatWritable), "org.datavec.api.writable.FloatWritable")>
    <JsonDerivedType(GetType(IntWritable), "org.datavec.api.writable.IntWritable")>
    <JsonDerivedType(GetType(LongWritable), "org.datavec.api.writable.LongWritable")>
    <JsonDerivedType(GetType(NullWritable), "org.datavec.api.writable.NullWritable")>
    <JsonDerivedType(GetType(Text), "org.datavec.api.writable.Text")>
    Public Interface Writable
        ''' <summary>
        ''' Serialize the fields of this object to <code>out</code>.
        ''' </summary>
        ''' <param name="out"> <code>DataOuput</code> to serialize this object into. </param>
        Sub write(ByVal [out] As DataOutput)

        ''' <summary>
        ''' Deserialize the fields of this object from <code>in</code>.
        '''
        ''' <para>For efficiency, implementations should attempt to re-use storage in the
        ''' existing object where possible.</para>
        ''' </summary>
        ''' <param name="in"> <code>DataInput</code> to deseriablize this object from. </param>
        Sub readFields(ByVal [in] As DataInput)

        ''' <summary>
        ''' Write the type (a single short value) to the DataOutput. See <seealso cref="WritableFactory"/> for details.
        ''' </summary>
        ''' <param name="out"> DataOutput to write to </param>
        Sub writeType(ByVal [out] As DataOutput)

        ''' <summary>
        ''' Convert Writable to double. Whether this is supported depends on the specific writable. </summary>
        Function toDouble() As Double

        ''' <summary>
        ''' Convert Writable to float. Whether this is supported depends on the specific writable. </summary>
        Function toFloat() As Single

        ''' <summary>
        ''' Convert Writable to int. Whether this is supported depends on the specific writable. </summary>
        Function toInt() As Integer

        ''' <summary>
        ''' Convert Writable to long. Whether this is supported depends on the specific writable. </summary>
        Function toLong() As Long

        ''' <summary>
        ''' Get the type of the writable.
        ''' </summary>
        Function [getType]() As WritableType

    End Interface

    ''' <summary>
    ''' Small numeric helpers the writables share. <c>fuzzyEquals</c> is Guava's
    ''' <c>DoubleMath.fuzzyEquals</c>; <c>signExtend</c> gives the Java (signed) value of a byte.
    ''' </summary>
    Friend Module WritableHelpers

        ''' <summary>Guava <c>DoubleMath.fuzzyEquals</c>: true if the values are within tolerance, equal, or both NaN.</summary>
        Public Function fuzzyEquals(ByVal a As Double, ByVal b As Double, ByVal tolerance As Double) As Boolean
            If tolerance < 0 OrElse Double.IsNaN(tolerance) Then
                Throw New ArgumentException("tolerance (" & tolerance & ") cannot be negative")
            End If
            Return Math.Abs(a - b) <= tolerance OrElse a = b OrElse (Double.IsNaN(a) AndAlso Double.IsNaN(b))
        End Function

        ''' <summary>The value of the byte as a Java <c>byte</c> (-128..127).</summary>
        Public Function signExtend(ByVal b As Byte) As Integer
            Return If(b > 127, CInt(b) - 256, CInt(b))
        End Function

        ''' <summary>Java <c>Arrays.hashCode(byte[])</c>: uses the signed byte values.</summary>
        Public Function javaByteArrayHash(ByVal a() As Byte) As Integer
            If a Is Nothing Then Return 0
            Dim result As Integer = 1
            For Each b As Byte In a
                result = 31 * result + signExtend(b)
            Next b
            Return result
        End Function

    End Module

End Namespace
